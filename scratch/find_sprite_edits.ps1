$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    $json = ConvertFrom-Json $line
    if ($json.tool_calls) {
        foreach ($tc in $json.tool_calls) {
            $file = $tc.args.TargetFile
            if (!$file) { $file = $tc.args.AbsolutePath }
            if ($file -match "SpriteGenerator.cs") {
                Write-Output "Step $i`: name=$($tc.name), file=$file"
            }
        }
    }
}
