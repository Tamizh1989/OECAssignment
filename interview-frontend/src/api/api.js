const api_url = "http://localhost:10010";

export const startPlan = async () => {
    const url = `${api_url}/Plan`;
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify({}),
    });

    if (!response.ok) throw new Error("Failed to create plan");

    return await response.json();
};

export const updateProcedureInPlan = async (planId, procedureId,isAdding) => {
    const url = `${api_url}/PlanProcedure/UpdateProcedureInPlan`;
    var command = { planId: planId, procedureId: procedureId, isAdding: isAdding };
    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(command),
    });

    if (!response.ok) throw new Error("Failed to update plan procedure");

    return true;
};

export const getProcedures = async () => {
    const url = `${api_url}/Procedures`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get procedures");

    return await response.json();
};

export const getPlanProcedures = async (planId) => {
    const url = `${api_url}/PlanProcedure?$filter=planId eq ${planId}&$expand=procedure`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get plan procedures");

    return await response.json();
};

export const getUsers = async () => {
    const url = `${api_url}/Users`;
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get users");

    return await response.json();
};

export const updateUsersInPlanProcedure = async (planId, procedureId, usersToAdd, usersToRemove) => {
    const url = `${api_url}/api/AssignUserToPlanProcedure/update`;
    const command = { planId, procedureId, usersToAdd, usersToRemove };

    const response = await fetch(url, {
        method: "POST",
        headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
        },
        body: JSON.stringify(command),
    });

    if (!response.ok) throw new Error("Failed to update users in plan procedure");

    return true;
};

export const getUserToPlanProcedures = async (planId,procedureId) => {
    const url = `${api_url}/api/AssignUserToPlanProcedure?$filter=planId eq ${planId} and procedureId eq ${procedureId}&$expand=User,PlanProcedure($expand=Procedure,Plan)`; 
    const response = await fetch(url, {
        method: "GET",
    });

    if (!response.ok) throw new Error("Failed to get user to plan procedures");

    return await response.json();
};



