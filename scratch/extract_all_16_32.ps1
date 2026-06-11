$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"

$steps = @(486, 490)
foreach ($s in $steps) {
    if ($s -lt (Get-Content $prevLogPath).Count) {
        $line = (Get-Content $prevLogPath)[$s]
        $json = ConvertFrom-Json $line
        $c = $json.content
        if (!$c -and $json.tool_calls) {
            $c = $json.tool_calls[0].args.CodeContent
            if (!$c) { $c = $json.tool_calls[0].args.TargetContent }
            if (!$c) { $c = $json.tool_calls[0].args.ReplacementContent }
        }
        if ($c) {
            $c | Out-File "scratch\prev_step_${s}_c.txt" -Encoding utf8
            Write-Output "Saved to scratch\prev_step_${s}_c.txt (length: $($c.Length))"
        }
    }
}
