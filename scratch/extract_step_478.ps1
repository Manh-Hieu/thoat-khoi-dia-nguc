$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $prevLogPath)[478]

$json = ConvertFrom-Json $line
Write-Output "Step 478 type: $($json.type)"
$c = $json.content
if ($c) {
    $c | Out-File "scratch\prev_step_478_c.txt" -Encoding utf8
    Write-Output "Saved to scratch\prev_step_478_c.txt (length: $($c.Length))"
} else {
    Write-Output "No content in step 478."
}
