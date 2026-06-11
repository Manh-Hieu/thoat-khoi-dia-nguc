$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $prevLogPath)[413]

Write-Output "Step 413 JSON length: $($line.Length)"
$json = ConvertFrom-Json $line
if ($json.tool_calls) {
    $tc = $json.tool_calls[0]
    Write-Output "Tool name: $($tc.name)"
    Write-Output "Args keys: $($tc.args | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name)"
    
    # Write the raw args JSON directly to a scratch file
    $tc.args | ConvertTo-Json -Depth 5 | Out-File "scratch\step_413_args_raw.txt" -Encoding utf8
    Write-Output "Saved raw args to scratch\step_413_args_raw.txt"
}
