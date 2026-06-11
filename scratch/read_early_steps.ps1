$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

# Loop through early lines and print details about each step
for ($i = 0; $i -lt 50; $i++) {
    if ($i -ge $lines.Count) { break }
    $line = $lines[$i]
    $json = ConvertFrom-Json $line
    Write-Output "Step $i`: type=$($json.type), status=$($json.status), source=$($json.source)"
}
