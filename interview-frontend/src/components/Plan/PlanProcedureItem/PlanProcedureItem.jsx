import React, { useState,useEffect } from "react";
import ReactSelect from "react-select";
import { updateUsersInPlanProcedure } from "../../../api/api";

const PlanProcedureItem = ({planId, procedure, users, assignedUsers }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    useEffect(() => {
        if (assignedUsers && assignedUsers.length > 0) {
            setSelectedUsers(assignedUsers.map(user => ({ label: user.label, value: user.value })));
        } else {
            setSelectedUsers([]);
        }
    }, [assignedUsers]);

    const handleAssignUserToProcedure = async(e) => {
     
        const newSelectedUsers = e || []; 
        setSelectedUsers(newSelectedUsers);

        const selectedUserIds = newSelectedUsers.map(option => option.value);
        const previousUserIds = selectedUsers.map(user => user.value);

        const usersToAdd = selectedUserIds.filter(id => !previousUserIds.includes(id));
        const usersToRemove = previousUserIds.filter(id => !selectedUserIds.includes(id));

        try 
        {
        if (usersToAdd.length > 0 || usersToRemove.length > 0) {
            await updateUsersInPlanProcedure(planId, procedure.procedureId, usersToAdd, usersToRemove);
            console.log("Users successfully updated for procedure");
        } 
        }
        catch (error) 
        {
            console.error("Error updating users for procedure:", error);
        }
    };

    
    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
