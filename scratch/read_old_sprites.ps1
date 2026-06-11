$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "GenerateMinhSprites") {
        Write-Output "Line $i matches. Length: $($line.Length). Type: $(if ($line -match '\"type\":\"[^\"]*\"') { $Matches[0] } else { 'unknown' })"
    }
}
