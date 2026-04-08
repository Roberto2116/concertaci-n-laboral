$dbPath = "C:\Users\Thinkpad\Desktop\Practicas\Proyecto_GRRLN_expediente\pemex\JR_NUEVAGENDA.accdb"
$connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;"
$connString16 = "Provider=Microsoft.ACE.OLEDB.16.0;Data Source=$dbPath;"

try {
    $conn = New-Object System.Data.OleDb.OleDbConnection($connString)
    $conn.Open()
    Write-Host "Connected using Microsoft.ACE.OLEDB.12.0"
} catch {
    Write-Host "Failed to connect with 12.0, trying 16.0..."
    try {
        $conn = New-Object System.Data.OleDb.OleDbConnection($connString16)
        $conn.Open()
        Write-Host "Connected using Microsoft.ACE.OLEDB.16.0"
    } catch {
        Write-Host "Failed to connect with 16.0. Exception: " $_.Exception.Message
        exit
    }
}

try {
    $schemaViews = $conn.GetSchema("Views")
    Write-Host "`n--- Views ---"
    $schemaViews | Format-Table TABLE_NAME, VIEW_DEFINITION -AutoSize

    # Try getting the queries from MSysObjects if Views schema is empty or doesn't have definitions.
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT Name, Type FROM MSysObjects WHERE Type=5 OR Type=1 OR Type=4"
    $reader = $cmd.ExecuteReader()
    Write-Host "`n--- Objects from MSysObjects ---"
    while ($reader.Read()) {
        Write-Host "Name: $($reader['Name']), Type: $($reader['Type'])"
    }
    $reader.Close()
} catch {
    Write-Host "Error getting schema or MSysObjects: $_"
}

$conn.Close()
