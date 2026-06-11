$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[22]

$json = ConvertFrom-Json $line
$c = $json.content
if (!$c -and $json.tool_calls) {
    $c = $json.tool_calls[0].args.CodeContent
}
if ($c) {
    $c | Out-File "scratch\original_sprite_generator.cs" -Encoding utf8
    Write-Output "Saved step 22 content to scratch\original_sprite_generator.cs"
} else {
    Write-Output "No content or tool call CodeContent in step 22. Checking step 23..."
    $line23 = (Get-Content $logPath)[23]
    $json23 = ConvertFrom-Json $line23
    if ($json23.content) {
        $json23.content | Out-File "scratch\original_sprite_generator.cs" -Encoding utf8
        Write-Output "Saved step 23 content to scratch\original_sprite_generator.cs"
    } else {
        Write-Output "Step 23 has no content either."
    }
}
