using HardDriveDelete;

Console.WriteLine("It is recommended to run this program after an operating system reset");
Console.WriteLine("If that has not been done, close this window, back up all important data,");
Console.WriteLine("and return the operating system to its default state, then run this program");
Console.WriteLine();
Console.WriteLine("If you are running this program on a non-OS drive, this step is not necessary.");
Console.WriteLine();

Console.WriteLine("Press Enter to proceed");
Console.ReadLine();

Console.Write("Please enter the drive letter(s) to wipe: ");
string? driveName = Console.ReadLine();
DriveInfo? driveInfo = null;

while (driveInfo is null)
{
	try
	{
		if (driveName is not null)
		{
			driveInfo = new DriveInfo(driveName);
		}
		else
		{
			throw new NullReferenceException($"{nameof(driveName)} was not properly set");
		}
	}
	catch
	{
		Console.Write("Please enter a valid drive letter(s): ");
		driveName = Console.ReadLine();
	}
}

long totalFreeSpace = driveInfo.TotalFreeSpace;
decimal tempSpace = totalFreeSpace;
int byteSuffix = 0;
while (tempSpace / 1024 >= 1.00m)
{
	byteSuffix++;
	tempSpace /= 1024;
}

Suffix byteDisplay = (Suffix)byteSuffix;

Console.WriteLine($"There are {tempSpace:F2} {byteDisplay}Bytes free space remaining on drive {driveName}");

// Slice free space into chunks of 11 files.
int totalFiles = 11;
long fileSize = totalFreeSpace / (totalFiles - 1);
int fileNumber = 0;
Guid fileName = Guid.NewGuid();

while (driveName is not null && totalFreeSpace > 0)
{
	FileStream tempFile = new FileStream(
		@$"{driveName}\{fileName}{fileNumber:D3}.nil",
		FileMode.Create,
		FileAccess.Write,
		FileShare.Read);

	Console.WriteLine($"File {fileNumber + 1} successfully created.");

	// refresh driveInfo
	driveInfo = new DriveInfo(driveName);
	totalFreeSpace = driveInfo.TotalFreeSpace;

	if (totalFreeSpace < fileSize)
	{
		fileSize = totalFreeSpace;
	}

	for (int i = 0; i < fileSize; i++)
	{
		tempFile.WriteByte(0);
	}

	Console.WriteLine($"Progress: {(decimal)totalFiles/fileNumber++:F2}%");
}

Console.WriteLine($"Hard drive is now full!  Please validate, then delete the *.nil files from {driveName}:\\");