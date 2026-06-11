$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $prevLogPath)[13]

$json = ConvertFrom-Json $line
Write-Output "Step 13 type: $($json.type)"
$c = $json.content
if (!$c -and $json.tool_calls) {
    $c = $json.tool_calls[0].args.CodeContent
    if (!$c) { $c = $json.tool_calls[0].args.TargetContent }
    if (!$c) { $c = $json.tool_calls[0].args.ReplacementContent }
}
if ($c) {
    $c | Out-File "scratch\step_13_c.txt" -Encoding utf8
    Write-Output "Saved to scratch\step_13_c.txt (length: $($c.Length))"
}
