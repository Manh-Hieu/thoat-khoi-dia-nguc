$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "GenerateMinhSprites") {
        $has1632 = $line -match "16,\s*32"
        $has2448 = $line -match "24,\s*48"
        Write-Output "Step $i matches. Length: $($line.Length). Has 16x32: $has1632. Has 24x48: $has2448"
    }
}
