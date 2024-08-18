let currentRoleId = "";

function OpenPopup(roleId, roleName) {
    currentRoleId = roleId;
    document.getElementById("deletePopup").style.display = "flex";
    document.getElementById("roleName").innerText = roleName;
}

document.getElementById("closePopup").onclick = function () {
    document.getElementById("deletePopup").style.display = "none";
};

document.getElementById("confirmDelete").onclick = function () {
    if (currentRoleId) {
        document.getElementById(`deleteForm-${currentRoleId}`).submit();
    }
    document.getElementById("deletePopup").style.display = "none";
};
