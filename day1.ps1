function Get-Fuel([int]$mass) {
    $fuel = [System.Math]::Floor($mass / [int]3) - 2
    
    if ($fuel -lt 0) {
        return 0
    }
    
    $fuel + (Get-Fuel $fuel)
}


$data = Get-Content day1.txt

$data | ForEach-Object {$total = 0} { 
    $mass = [int]::Parse($_)
    $fuel = Get-Fuel $mass
    $total += $fuel
} {$total}

# Part 1: 3427947
# Part 2: 5139037