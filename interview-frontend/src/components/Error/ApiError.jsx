import React, { useState } from "react";

export default function ApiError(props) {
    return (
        <>
            <div className="popup">
                <div class="alert alert-danger" role="alert">
                    <p>An API Error occured</p>
                    <button class="btn btn-primary" onClick={() => props.callback(false)}>Close</button>
                </div>
            </div>
        </>
    );
}
