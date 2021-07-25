CREATE TABLE MetroplexZero.GeneralMetrics (
	MetricId INT IDENTITY(1,1) PRIMARY KEY,
	RowLoadedDateTime DATETIME2(7),
	GameVersion varchar(255),
	InstallId varchar(36),
	RunId varchar(36),
	EventType varchar(128),
	EventData varchar(2048)
)
