$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

$steps = @(325, 341, 355, 435, 470, 473, 505)
foreach ($s in $steps) {
    if ($s -lt $lines.Count) {
        $line = $lines[$s]
        $json = ConvertFrom-Json $line
        $c = $json.content
        if ($c) {
            # Check if this contains GenerateMinhSprites
            if ($c -match "GenerateMinhSprites") {
                # Save to a file
                $c | Out-File "scratch\prev_view_${s}_content.txt" -Encoding utf8
                Write-Output "Saved step $s content (length: $($c.Length))"
            }
        }
    }
}
