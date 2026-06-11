$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[31]

Write-Output "Step 31 JSON length: $($line.Length)"
$json = ConvertFrom-Json $line
if ($json.tool_calls) {
    $tc = $json.tool_calls[0]
    Write-Output "Tool name: $($tc.name)"
    if ($tc.args.TargetContent) {
        Write-Output "TargetContent length: $($tc.args.TargetContent.Length)"
        [System.IO.File]::WriteAllText("scratch\step_31_t.txt", $tc.args.TargetContent)
    }
    if ($tc.args.ReplacementContent) {
        Write-Output "ReplacementContent length: $($tc.args.ReplacementContent.Length)"
        [System.IO.File]::WriteAllText("scratch\step_31_r.txt", $tc.args.ReplacementContent)
    }
} else {
    Write-Output "No tool_calls in step 31."
}
