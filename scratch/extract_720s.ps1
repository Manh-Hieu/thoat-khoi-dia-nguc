$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"

$steps = @(722, 727, 728)
foreach ($s in $steps) {
    if ($s -lt (Get-Content $logPath).Count) {
        $line = (Get-Content $logPath)[$s]
        $json = ConvertFrom-Json $line
        $c = $json.content
        if (!$c -and $json.tool_calls) {
            $c = $json.tool_calls[0].args.CodeContent
        }
        if ($c) {
            $c | Out-File "scratch\step_$s.txt" -Encoding utf8
            Write-Output "Saved step $s code/content to scratch\step_$s.txt"
        }
    }
}
