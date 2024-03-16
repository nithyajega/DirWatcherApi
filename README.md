1. Create a database name DirWatcherDb in MSSQL Server.

2. Run the below Sql query to create two tables

    a.) CREATE TABLE FileChanges ( Id INT PRIMARY KEY IDENTITY, ChangeType NVARCHAR(50) NOT NULL, FilePath NVARCHAR(255) NOT NULL, ChangeTime DATETIME NOT NULL );

    b)Create Table OccurrenceLogs ( Id INT PRIMARY KEY IDENTITY, OccurrenceCount INT NOT NULL, Logtime DATETIME NOT NULL, FilePath NVARCHAR(255) NOT NULL, MagicString NVARCHAR(50) NOT NULL );

3. FileChanges - In the given Directory if any File is added / deleted /modified those details will be logged in this table.

4. OccurrenceLogs - In the existing / added file the count of magic string is logged.

5. Download the API from the branch. In appsettings.json -> change "DirectoryPath" and "your_magic_string" according to the directory and the magicstring you are going to test.

6. Now run the application and try to add a new file in the given directory.

7. Open the SQL server and run the below queries

    Select * from FileChanges

    Select * from OccurrenceLogs

8. The FileChanges table will be updated with the changes made in the directory and in the OccurrenceLogs table, if the given magic string exists it will update the count of occurrences.

9. From the API we can "Start" and "Stop" the task manually.

10. To Search for a different magic string other than the given one we can manually enter in the "Search".
