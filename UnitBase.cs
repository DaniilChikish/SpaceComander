Enum UnitType = {InfintryVehikle; ReconVehicle; Tank)
Dictionary<UnitType, string> UnitStats = {(InfintryVehikle, ); (ReconVehicle, ); (Tank, )}

class Unit
{
//base varibles
public UnitType;
public Object GraficUnitRef;

//depend varibles
public int attack;
public int health;
public double accurancy;
public double stealthness;
//independ varibles
public int brusts;
public int ammo;
public int speed;

public Unit(UnitType type)
{
swich (type)
{
case InfintryVehikle:
{
break;
}
case ReconVehicle:
{
break;
}
case Tank:
{
break;
}
}
}
}