import React from "react";

const ProcedureItem = ({ procedure, handleUpdateProcedureInPlan, planProcedures }) => {
    // Check if the procedure is already part of the plan
    const isProcedureInPlan = planProcedures.find(
        (p) => p.procedureId === procedure.procedureId
    );


    const handleCheckboxChange = () => {
        handleUpdateProcedureInPlan(procedure, !isProcedureInPlan);
    };

    return (
        <div className="py-2">
            <div className="form-check">
                <input
                    className="form-check-input"
                    type="checkbox"
                    value=""
                    id={`procedureCheckbox-${procedure.procedureId}`} 
                    checked={isProcedureInPlan}  
                    onChange={handleCheckboxChange}  
                />
                <label className="form-check-label" htmlFor={`procedureCheckbox-${procedure.procedureId}`}>
                    {procedure.procedureTitle}
                </label>
            </div>
        </div>
    );
};

export default ProcedureItem;
