Get-Content day1.txt | ForEach-Object {$total = 0} { 
    $mass = [int]::Parse($_)
    $fuel = [System.Math]::Floor($mass / [int]3) - 2
    $total += $fuel } {$total}

# Part 1: 3427947