$jsonPath = "scratch\found_1632_relaxed_0763989a-cbb6-41f3-b37c-454ec0d14630_1134.txt"
$line = Get-Content $jsonPath
$json = ConvertFrom-Json $line

$c = $json.content
if ($c) {
    $c | Out-File "scratch\full_old_diff.txt" -Encoding utf8
    Write-Output "Saved full diff content to scratch\full_old_diff.txt (length: $($c.Length))"
} else {
    Write-Output "No content field in step 1134."
}
