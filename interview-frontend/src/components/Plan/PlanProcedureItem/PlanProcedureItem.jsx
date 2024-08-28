import React, { useState } from "react";
import ReactSelect from "react-select";
import {
    MapUserProcedureToPlan,
    deleteProcedureToPlan
} from "../../../api/api";
import ApiError from "../../Error/ApiError"
const PlanProcedureItem = ({ procedure, users, planId, selectedUserList }) => {
    const [selectedUsers, setSelectedUsers] = useState(users.filter(x => selectedUserList.some(y => y.userId == x.value)));
    const [error, setError] = useState(false);

    const handleAssignUserToProcedure = async (e) => {
        const removedUsers = selectedUsers.filter(x => !e.includes(x));
        const addedUsers = e.filter(x => !selectedUsers.includes(x));
        const mergedArray = [...removedUsers, ...addedUsers];

        let result = true;

        try {
            if (removedUsers.length === 0) {
                result = await MapUserProcedureToPlan(
                    planId,
                    procedure.procedureId,
                    mergedArray.length === 1 ? mergedArray[0].value : 0,
                    false
                );
            } else {
                result = await deleteProcedureToPlan(
                    planId,
                    procedure.procedureId,
                    mergedArray.length === 1 ? mergedArray[0].value : 0,
                    true
                );
            }

            setError(!result);

            if (result) {
                setSelectedUsers(e);
            }
        } catch (error) {
            console.error("An error occurred:", error);
            setError(true);
        }
    };
    const handleError = (data) => {
        setError(data);
    };
    return (
        <div>
            {!error ? (
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
                </div>) : <><ApiError callback={handleError}></ApiError>
            </>}
        </div>
    );
};

export default PlanProcedureItem;
