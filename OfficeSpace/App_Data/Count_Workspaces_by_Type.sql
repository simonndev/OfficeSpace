SELECT LName, BName, FName, UName, -- non-pivot field(s)
	[1] AS NumberOfWorkstations, [2] AS NumberOfLabs, [3] AS NumberOfMeetingRooms
FROM
(
SELECT L.LocationName AS LName
	, B.BuildingName AS BName
	, F.FloorName AS FName
	, U.UnitName AS UName
	, WS.WorkspaceId, WS.WorkspaceTypeId -- columns being aggregated
	FROM Locations L
		JOIN Buildings B ON L.LocationId = B.LocationId AND B.IsDeleted = 0
		JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
		JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
		JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
	WHERE L.IsDeleted = 0
) Src
PIVOT (
	-- aggregation function
	COUNT (WorkspaceId) FOR WorkspaceTypeId IN ( [1], [2], [3] )
) Pvt

-- Location
SELECT ID, Name, -- non-pivot field(s)
	[1] AS NumberOfWorkstations, [2] AS NumberOfLabs, [3] AS NumberOfMeetingRooms
FROM
(
SELECT L.LocationId AS ID, L.LocationName AS Name
	, WS.WorkspaceId, WS.WorkspaceTypeId -- columns being aggregated
	FROM Locations L
		JOIN Buildings B ON L.LocationId = B.LocationId AND B.IsDeleted = 0
		JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
		JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
		JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
	WHERE L.IsDeleted = 0
) Src
PIVOT (
	-- aggregation function
	COUNT (WorkspaceId) FOR WorkspaceTypeId IN ( [1], [2], [3] )
) Pvt

-- Building
SELECT ID, Name, Code, -- non-pivot field(s)
	[1] AS NumberOfWorkstations, [2] AS NumberOfLabs, [3] AS NumberOfMeetingRooms
FROM
(
SELECT B.BuildingID AS ID, B.BuildingCode AS Code, B.BuildingName AS Name
	, WS.WorkspaceId, WS.WorkspaceTypeId -- columns being aggregated
	FROM Buildings B
		JOIN Floors F ON B.BuildingId = F.BuildingID AND F.IsDeleted = 0
		JOIN Units U ON F.FloorId = U.FloorID AND U.IsDeleted = 0
		JOIN Workspaces WS ON U.UnitId = WS.UnitId AND WS.IsDeleted = 0
	WHERE B.IsDeleted = 0
) Src
PIVOT (
	-- aggregation function
	COUNT (WorkspaceId) FOR WorkspaceTypeId IN ( [1], [2], [3] )
) Pvt
