$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"

$steps = @(549, 557)
foreach ($s in $steps) {
    if ($s -lt (Get-Content $logPath).Count) {
        $line = (Get-Content $logPath)[$s]
        $json = ConvertFrom-Json $line
        Write-Output "Step $s tool: $($json.tool_calls[0].name), length: $($line.Length)"
        if ($json.tool_calls) {
            $tc = $json.tool_calls[0]
            if ($tc.args.TargetContent) {
                [System.IO.File]::WriteAllText("scratch\step_${s}_t.txt", $tc.args.TargetContent)
            }
            if ($tc.args.ReplacementContent) {
                [System.IO.File]::WriteAllText("scratch\step_${s}_r.txt", $tc.args.ReplacementContent)
                Write-Output "Saved step $s replacement content."
            }
        }
    }
}
