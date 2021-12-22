using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    private Dictionary<int, Unit> attackableUnits;
    private float radius;
    private UnitManager manager;
    private Ship ship;

    private void Awake()
    {
        attackableUnits = new Dictionary<int, Unit>();
        radius = GetComponent<SphereCollider>().radius;
        ship = GetComponentInParent<Ship>();
        manager = ship.Manager();
    }

    private void Update()
    {
        if (!ship.IsInCombat() && attackableUnits.Count > 0)
        {
            attackableUnits = RemoveDestroyed();
            List<Unit> units = attackableUnits.Values.ToList();

            if (units.Count > 0)
            {
                UnitsUtil.SortUnitsByDistance(units, transform.position);
                ship.Attack(units[0], true); // can be false also.... may depend on ship or something
            }
        }
    }

    private Dictionary<int, Unit> RemoveDestroyed()
    {
        Dictionary<int, Unit> units = new Dictionary<int, Unit>();

        foreach (var pair in attackableUnits)
            if (pair.Value != null && pair.Value.IsAlive())
                units.Add(pair.Key, pair.Value);

        return units;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null)
            if (!manager.Contains(unit))
                attackableUnits.Add(unit.Id(), unit);
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();

        if (unit != null)
            if (attackableUnits.ContainsKey(unit.Id()))
                attackableUnits.Remove(unit.Id());
    }

    public Unit[] GetUnitsInside()
    {
        return attackableUnits.Values.ToArray();
    }

    public bool IsOutside(Unit unit)
    {
        return !attackableUnits.ContainsKey(unit.Id());
    }

    public float GetRadius()
    {
        return radius;
    }
}