$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

$count = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "Minh_Idle_Down") {
        Write-Output "Step $i matches Minh_Idle_Down! Length: $($line.Length)"
        # Save step content to scratch
        $line | Out-File "scratch\prev_raw_step_$i.txt" -Encoding utf8
        $count++
        if ($count -ge 10) { break }
    }
}
