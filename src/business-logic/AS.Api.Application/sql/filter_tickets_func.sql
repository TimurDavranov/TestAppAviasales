drop function if exists FilterTickets;
create function FilterTickets(@search varchar(250), @prop varchar(250), @isAsc bit, @skip int, @take int)
    returns table
as
return
(
    with Tickets as (
        select
            0 as TotalCount,
            at.Id as TicketId,
            0 as Source,
            at.DepartureDate,
            CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name) as DeparturePlace,
            ISNULL((
                select top 1 CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name)
                from externals.amadeus_ticket_destinations atd
                inner join [references].countries on countries.ISOCode3 = atd.CountryCode
                inner join [references].cities on cities.Id = atd.CityId
                inner join [references].airports on airports.Code = atd.AirportCode
                where atd.AmadeusTicketId = at.Id
                order by atd.[Order] desc
            ), '') as DestinationPlace,
            ISNULL((
                select top 1 atd.DestinationDate
                from externals.amadeus_ticket_destinations atd
                where atd.AmadeusTicketId = at.Id
                order by atd.[Order] desc
            ), at.DepartureDate) as DestinationDate,
            (
                select count(*)
                from externals.amadeus_ticket_destinations atd
                where atd.AmadeusTicketId = at.Id
            ) as TransferCount,
            at.Price as TicketPrice,
            at.NumberOfSeats as TicketCount
        from externals.amadeus_tickets at
        inner join [references].countries on countries.ISOCode3 = at.CountryCode
        inner join [references].cities on cities.Id = at.CityId
        inner join [references].airports on airports.Code = at.AirportCode
        where at.DepartureDate like '%' + @search + '%'
              or CAST(at.Price as varchar(max)) like '%' + @search + '%'
              or CAST((
                select count(*)
                from externals.amadeus_ticket_destinations atd
                where atd.AmadeusTicketId = at.Id
              ) as varchar(max)) like '%' + @search + '%'
              or CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name) like '%' + @search + '%'
        union all
        select
            0 as TotalCount,
            st.Id as TicketId,
            1 as Source,
            st.DepartureDate,
            CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name) as DeparturePlace,
            ISNULL((
                select top 1 CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name)
                from externals.skyscanner_ticket_destinations std
                inner join [references].countries on countries.ISOCode3 = std.CountryCode
                inner join [references].cities on cities.Id = std.CityId
                inner join [references].airports on airports.Code = std.AirportCode
                where std.SkyscannerTicketId = st.Id
                order by std.[Order] desc
            ), '') as DestinationPlace,
            ISNULL((
                select top 1 std.DestinationDate
                from externals.skyscanner_ticket_destinations std
                where std.SkyscannerTicketId = st.Id
                order by std.[Order] desc
            ), st.DepartureDate) as DestinationDate,
            (
                select count(*)
                from externals.skyscanner_ticket_destinations std
                where std.SkyscannerTicketId = st.Id
            ) as TransferCount,
            st.Price as TicketPrice,
            st.NumberOfSeats as TicketCount
        from externals.skyscanner_tickets st
        inner join [references].countries on countries.ISOCode3 = st.CountryCode
        inner join [references].cities on cities.Id = st.CityId
        inner join [references].airports on airports.Code = st.AirportCode
        where st.DepartureDate like '%' + @search + '%'
              or CAST(st.Price as varchar(max)) like '%' + @search + '%'
              or CAST((
                select count(*)
                from externals.skyscanner_ticket_destinations std
                where std.SkyscannerTicketId = st.Id
              ) as varchar(max)) like '%' + @search + '%'
              or CONCAT(countries.Name, ' ', cities.Name, ' ', airports.Name) like '%' + @search + '%'
    ),
    FilteredTickets as (
        select
            *,
            ROW_NUMBER() over (
                order by
                    case when @prop = 'TicketPrice' then TicketPrice end,
                    case when @prop = 'TransferCount' then TransferCount end,
                    case when @prop = 'DeparturePlace' then DeparturePlace end
            ) as RowNum
        from Tickets
    )
    select
        (select count(*) from FilteredTickets) as TotalCount,
        TicketId,
        Source,
        DepartureDate,
        DeparturePlace,
        DestinationPlace,
        DestinationDate,
        TransferCount,
        TicketPrice,
        TicketCount
    from FilteredTickets
    where RowNum > @skip
    and RowNum <= (@skip + @take)
);
