import React, { useState } from "react";
import ReactSelect from "react-select";
import {
    MapUserProcedureToPlan
} from "../../../api/api";

const PlanProcedureItem = ({ procedure, users, planId, selectedUserList }) => {
    const [selectedUsers, setSelectedUsers] = useState(users.filter(x => selectedUserList.some(y => y.userId == x.value)));

    const handleAssignUserToProcedure = async (e) => {
        var removeduser = selectedUsers.filter(x => !e.includes(x));
        var addeduser = e.filter(x => !selectedUsers.includes(x));
        const mergedArray = [...removeduser, ...addeduser];
        setSelectedUsers(e);

        await MapUserProcedureToPlan(planId, procedure.procedureId, mergedArray.length == 1 ? mergedArray[0].value : 0, removeduser.length == 0 ? false : true);
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
