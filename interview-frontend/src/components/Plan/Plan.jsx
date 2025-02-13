import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  getPlanProcedures,
  getProcedures,
  getUsers,
  getUserToPlanProcedures,
  updateProcedureInPlan
} from "../../api/api";
import Layout from '../Layout/Layout';
import ProcedureItem from "./ProcedureItem/ProcedureItem";
import PlanProcedureItem from "./PlanProcedureItem/PlanProcedureItem";

const Plan = () => {
  let { id } = useParams();
  const [procedures, setProcedures] = useState([]);
  const [planProcedures, setPlanProcedures] = useState([]);
  const [users, setUsers] = useState([]);
  const [assignedUsers, setAssignedUsers] = useState({});

  useEffect(() => {
    (async () => {
      var procedures = await getProcedures();
      var planProcedures = await getPlanProcedures(id);
      var users = await getUsers();

      var userOptions = [];
      users.map((u) => userOptions.push({ label: u.name, value: u.userId }));

      setUsers(userOptions);
      setProcedures(procedures);
      setPlanProcedures(planProcedures);

      const assignedUsersData = {};
      for (const planProcedure of planProcedures) {
        const usersForProcedure = await getUserToPlanProcedures(id, planProcedure.procedureId);
        console.log("Users for procedure:", planProcedure.procedureId, usersForProcedure);
        assignedUsersData[planProcedure.procedureId] = usersForProcedure.map((user) => ({
          label: user.user.name,
          value: user.user.userId,
        }));
      }
      setAssignedUsers(assignedUsersData);
    })();
  }, [id]);
   
  const handleUpdateProcedureInPlan = async (procedure, isAdding) => {
    try {
      await updateProcedureInPlan(id, procedure.procedureId, isAdding);

        setPlanProcedures((prevState) => {
            if (isAdding) {
                if (!prevState.some((p) => p.procedureId === procedure.procedureId)) {
                    return [
                        ...prevState,
                        {
                            planId: id,
                            procedureId: procedure.procedureId,
                            procedure: {
                                procedureId: procedure.procedureId,
                                procedureTitle: procedure.procedureTitle,
                            },
                        },
                    ];
                }
            } else {
                return prevState.filter((p) => p.procedureId !== procedure.procedureId);
            }
            return prevState;
        });

        setAssignedUsers((prevUsers) => {
            if (isAdding) {
                return {
                    ...prevUsers, 
                    [procedure.procedureId]: prevUsers[procedure.procedureId] || [] 
                };
            } else {
                const updatedUsers = { ...prevUsers };
                delete updatedUsers[procedure.procedureId]; 
                return updatedUsers;
            }
        });
      } catch (error) {
        console.error("Error updating procedure in plan:", error);
    }
};

  return (
    <Layout>
      <div className="container pt-4">
        <div className="d-flex justify-content-center">
          <h2>OEC Interview Frontend</h2>
        </div>
        <div className="row mt-4">
          <div className="col">
            <div className="card shadow">
              <h5 className="card-header">Repair Plan</h5>
              <div className="card-body">
                <div className="row">
                  <div className="col">
                    <h4>Procedures</h4>
                    <div>
                      {procedures.map((p) => (
                        <ProcedureItem
                          key={p.procedureId}
                          procedure={p}
                          handleUpdateProcedureInPlan={handleUpdateProcedureInPlan}
                          planProcedures={planProcedures}
                        />
                      ))}
                    </div>
                  </div>
                  <div className="col">
                    <h4>Added to Plan</h4>
                    <div>
                      {planProcedures.map((p) => (
                        <PlanProcedureItem
                          planId={p.planId} 
                          key={p.procedure.procedureId}
                          procedure={p.procedure}
                          users={users}
                          assignedUsers={assignedUsers[p.procedure.procedureId] || []}
                        />
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default Plan;
