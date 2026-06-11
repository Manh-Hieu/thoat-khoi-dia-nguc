$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "GenerateMinhSprites") {
        # Check if it contains 16, 32 or 24, 48
        $has1632 = $line -match "16,\s*32"
        $has2448 = $line -match "24,\s*48"
        Write-Output "Step $i matches. Length: $($line.Length). Has 16x32: $has1632. Has 24x48: $has2448"
    }
}
