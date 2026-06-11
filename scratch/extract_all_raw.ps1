$convoIds = @("98871e4d-8463-4de3-9b6e-e8e5d4ff6662", "738071b7-3a6f-4959-95f9-4dab128e550b")

$count = 0
foreach ($id in $convoIds) {
    $logPath = "C:\Users\hieua\.gemini\antigravity\brain\$id\.system_generated\logs\transcript.jsonl"
    if (Test-Path $logPath) {
        $lines = Get-Content $logPath
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match "GenerateMinhSprites" -and $line -match "16,\s*32") {
                # This line contains the 16x32 version!
                # Let's save the raw line to scratch
                $line | Out-File "scratch\raw_1632_step_${id}_$i.txt" -Encoding utf8
                Write-Output "Saved raw step $i from convo $id to scratch\raw_1632_step_${id}_$i.txt (length: $($line.Length))"
                $count++
            }
        }
    }
}
