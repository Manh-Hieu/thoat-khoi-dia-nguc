$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[72]

$json = ConvertFrom-Json $line
if ($json.tool_calls) {
    $tc = $json.tool_calls[0]
    Write-Output "Tool: $($tc.name)"
    # Write the target content and replacement content to files
    $tc.args.TargetContent | Out-File "scratch\step_72_target.txt" -Encoding utf8
    $tc.args.ReplacementContent | Out-File "scratch\step_72_replacement.txt" -Encoding utf8
    Write-Output "Saved target and replacement content of step 72."
} else {
    Write-Output "No tool_calls in step 72."
}
