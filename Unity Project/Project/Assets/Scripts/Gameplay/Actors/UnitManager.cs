using UnityEngine;
using System.Collections.Generic;

namespace Gem
{
    public class UnitManager : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            //If right click, Issue Order Based On Condition
            //
            //Condition A - Hit Actor is a Enemy Unit, Order Unit Selection to Attack
            //Condition B - Hit Actor is a Friendly Unit, Order Unit Selection to Follow
            //Condition C - Hit Actor is a Enemy Structure , Order Unit Selection To Move to Unit
            //Condition D - Hit Actor is a Friendly Structure, Order Unit Selection To Move to Structure
            if (Input.GetMouseButtonDown(Constants.INPUT_ISSUE_ORDER))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray,out hit, 100.0f))
                {
                    Actor actor = hit.collider.GetComponent<Actor>();
                    if(actor != null)
                    {
                        Unit unit = actor as Unit;
                        if(unit != null)
                        {
                            List<Unit> unitSelection = GameSelection.GetSelectedUnits();
                            foreach (Unit enumeratedUnit in unitSelection)
                            {
                                enumeratedUnit.InvokeActorEvent(new ActorEvent(ActorEventType.IssueOrderFollow, unit));
                            }
                        }
                        else
                        {
                            //TODO: Check Structure
                        }
                    }
                    else
                    {
                        List<Unit> unitSelection = GameSelection.GetSelectedUnits();
                        foreach(Unit enumeratedUnit in unitSelection)
                        {
                            enumeratedUnit.InvokeActorEvent(new ActorEvent(ActorEventType.IssueOrderMove, hit.point));
                        }
                    }

                }
            }
            else if(Input.GetKeyDown(Constants.INPUT_STOP))
            {
                List<Unit> unitSelection = GameSelection.GetSelectedUnits();
                foreach(Unit unit in unitSelection)
                {
                    unit.InvokeActorEvent(new ActorEvent(ActorEventType.IssueOrderStop, null));
                }
            }

        }
    }
}


