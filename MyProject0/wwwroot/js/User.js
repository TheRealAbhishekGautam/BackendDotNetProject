var dataTable
$(document).ready(function () {
    LoadDataTable();
});
function LoadDataTable(){
   dataTable = $('#TableData').DataTable({
        "ajax": { url : '/admin/user/getall'},
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "20%" },
            { data: 'phoneNumber', "width": "12%" },
            { data: 'company.name', "width": "8%" },
            { data: "role", "width": "10%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd"},
                "render": function (data) {

                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `<div class="text-center">
                                    <a onclick = LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:120px;">
                                        <i class="bi bi-unlock-fill"></i> Unlock
                                    </a>
                                    <a href="/admin/user/rolemanagement?userid=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:120px;">
                                        Permission
                                    </a>
                                </div>`
                    }
                    else {
                        return `<div class="text-center">
                                    <a onclick = LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:120px;">
                                        <i class="bi bi-lock-fill"></i> Lock
                                    </a>
                                    <a href="/admin/user/rolemanagement?userid=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:120px;">
                                        Permission
                                    </a>
                                </div>`
                    }
                },
                "width":"23%"
            }
        ]
    });
}
function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/Admin/User/LockUnlockAccount',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}