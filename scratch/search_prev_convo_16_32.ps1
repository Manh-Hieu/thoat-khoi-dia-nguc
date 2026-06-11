$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "16" -and $line -match "32" -and $line -match "Minh_Idle") {
        Write-Output "Step $i matches 16, 32 and Minh_Idle! Length: $($line.Length)"
        # Save to scratch
        $line | Out-File "scratch\prev_step_16_32_$i.txt" -Encoding utf8
    }
}
