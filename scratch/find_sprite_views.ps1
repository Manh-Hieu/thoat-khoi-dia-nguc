$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

$count = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    $json = ConvertFrom-Json $line
    if ($json.type -eq "VIEW_FILE") {
        # Check if the path is SpriteGenerator.cs
        if ($json.tool_calls) {
            $path = $json.tool_calls[0].args.AbsolutePath
            if ($path -match "SpriteGenerator.cs") {
                Write-Output "Step $i viewed SpriteGenerator.cs!"
                # Save the content to scratch
                $content = $json.content
                if ($content) {
                    $content | Out-File "scratch\original_viewed_sprites_$i.txt" -Encoding utf8
                    Write-Output "Saved to scratch\original_viewed_sprites_$i.txt (length: $($content.Length))"
                }
            }
        }
    }
}
