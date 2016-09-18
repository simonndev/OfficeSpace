--DROP PROCEDURE [USP_CountAllTypeOfWorkspace]

CREATE PROCEDURE [USP_CountAllTypeOfWorkspace]
    -- Add the parameters for the stored procedure here
	@LocationID int, -- All(0) Locations
	@BuildingID int, -- All(0)/one Building(s) from a specific Location
    @FloorID int, -- All(0)/one Floor(s) a specific Building
	@UnitID int -- All(0)/one Unit(s) from a specific Floor
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

	-- [1], [2], [3], ..., [n]
	DECLARE @wsTypeIdCols AS NVARCHAR(MAX) 
	SELECT @wsTypeIdCols = STUFF(
		(
			SELECT ', ' + QUOTENAME(TypeId)
			FROM WorkspaceTypes
			WHERE IsDeleted = 0
			ORDER BY TypeId
			FOR XML PATH(''), TYPE
		).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
	--PRINT @wsTypeIdCols

	-- [1] AS [NumberOfWorkstations], [2] AS [NumberOfLabs], [3] AS [NumberOfMeetingRooms], ..., [n] AS [NumberOfLasts]
	DECLARE @wsTypeIdAsNameCols AS NVARCHAR(MAX)
	SELECT @wsTypeIdAsNameCols = STUFF(
	(
		SELECT ', ' + QUOTENAME(TypeId) + ' AS ' + QUOTENAME('NumberOf' + REPLACE( LTRIM(RTRIM(TypeName)), ' ', '' ) + 's') -- TypeID
		FROM WorkspaceTypes
		WHERE IsDeleted = 0
		ORDER BY TypeId
		FOR XML PATH(''), TYPE
	).value('.', 'NVARCHAR(MAX)'), 1, 1, '')
	--PRINT @wsTypeIdAsNameCols

	DECLARE @selectFromSource AS NVARCHAR(MAX)

	IF @LocationId = 0
		SELECT @selectFromSource = '
			SELECT L.LocationId AS ItemId, L.LocationName AS Name,
				WS.WorkspaceId, WS.WorkspaceTypeId
			FROM Locations L
				JOIN Buildings B ON L.LocationId = B.LocationId AND B.IsDeleted = 0
				JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
				JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
				JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
			WHERE L.IsDeleted = 0'
	ELSE
	BEGIN
		SELECT @selectFromSource = '
			SELECT L.LocationId AS ItemId, L.LocationName AS Name,
				WS.WorkspaceId, WS.WorkspaceTypeId
			FROM Locations L
				JOIN Buildings B ON L.LocationId = B.LocationId AND B.IsDeleted = 0
				JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
				JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
				JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
			WHERE L.IsDeleted = 0 AND L.LocationID = ' + CAST(@LocationId as nvarchar(5))
		
		IF @BuildingID = 0
			SELECT @selectFromSource = '
				SELECT B.BuildingId AS ItemId, B.BuildingName AS Name,
					WS.WorkspaceId, WS.WorkspaceTypeId
				FROM Buildings B
					JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
					JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
					JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
				WHERE B.IsDeleted = 0'
		ELSE IF @BuildingID > 0
		BEGIN
			SELECT @selectFromSource = '
				SELECT B.BuildingId AS ItemId, B.BuildingName AS Name,
					WS.WorkspaceId, WS.WorkspaceTypeId
				FROM Buildings B
					JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
					JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
					JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
				WHERE B.IsDeleted = 0 AND B.BuildingId = ' + CAST(@BuildingID as nvarchar(5))

			IF @FloorID = 0
				SELECT @selectFromSource = '
					SELECT F.FloorId AS ItemId, F.FloorName AS Name,
						WS.WorkspaceId, WS.WorkspaceTypeId
					FROM Floors F
						JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
						JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
					WHERE F.IsDeleted = 0'
			ELSE IF @FloorID > 0
			BEGIN
				SELECT @selectFromSource = '
					SELECT F.FloorId AS ItemId, F.FloorName AS Name,
						WS.WorkspaceId, WS.WorkspaceTypeId
					FROM Floors F
						JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
						JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
					WHERE F.IsDeleted = 0 AND F.FloorId = ' + CAST(@FloorID as nvarchar(5))

				IF @UnitID = 0
					SELECT @selectFromSource = '
						SELECT U.UnitId AS ItemId, U.UnitName AS Name,
							WS.WorkspaceId, WS.WorkspaceTypeId
						FROM Units U
							JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
						WHERE U.IsDeleted = 0'
				ELSE IF @UnitID > 0
					SELECT @selectFromSource = 
						N'SELECT U.UnitId AS ItemId, U.UnitName AS Name,
							WS.WorkspaceId, WS.WorkspaceTypeId
						FROM Units U JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
						WHERE U.IsDeleted = 0 AND U.UnitId = ' + CAST(@UnitID AS nvarchar(5))
			END
		END
	END

	DECLARE @query AS NVARCHAR(MAX)
	SELECT @query = '
		SELECT ItemId, Name, ' + @wsTypeIdAsNameCols + '
		FROM
		('
			+ @selectFromSource +
		') Src
		PIVOT
		(
			-- aggregation function
			COUNT (WorkspaceId) FOR WorkspaceTypeId IN (' + @wsTypeIdCols + ')
		) Pvt
	'
	
	--PRINT @query
	EXEC(@query)

END