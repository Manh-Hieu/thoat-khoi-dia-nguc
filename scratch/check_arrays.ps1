$content = Get-Content "Assets/Editor/SpriteGenerator.cs"
$arrays = @("down", "down_walk1", "down_walk2", "up", "up_walk1", "up_walk2", "left", "left_walk1", "left_walk2", "right", "right_walk1", "right_walk2")

foreach ($arr in $arrays) {
    $found = $false
    $count = 0
    foreach ($line in $content) {
        if ($line -match "string\[\] $arr = \{") {
            $found = $true
            continue
        }
        if ($found) {
            if ($line -match "\};") {
                $found = $false
                Write-Output "$arr count: $count"
            } else {
                $count++
            }
        }
    }
}
