$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[692]

# The JSON contains a "content" or "tool_calls" field
$json = ConvertFrom-Json $line
# Let's check if there's a tool_call
if ($json.tool_calls) {
    $c = $json.tool_calls[0].args.CodeContent
    if ($c) {
        $c | Out-File "scratch\step_692_code.txt" -Encoding utf8
        Write-Output "Saved step 692 tool call code."
    }
}
if ($json.content) {
    $json.content | Out-File "scratch\step_692_content.txt" -Encoding utf8
    Write-Output "Saved step 692 content."
}
