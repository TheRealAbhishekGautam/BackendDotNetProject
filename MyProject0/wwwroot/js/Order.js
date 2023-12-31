var dataTable
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        LoadDataTable("inprocess");
    }
    else {
        if (url.includes("pending")) {
            LoadDataTable("pending");
        }
        else {
            if (url.includes("completed")) {
                LoadDataTable("completed");
            }
            else {
                if (url.includes("approved")) {
                    LoadDataTable("approved");
                }
                else {
                    LoadDataTable("all");
                }
            }
        }
    }
        
});

function LoadDataTable(status){
    dataTable = $('#TableData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "8%" },
            { data: 'name', "width": "20%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "25%" },
            { data: 'orderStatus', "width": "12%" },
            { data: 'orderTotal', "width": "12%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                    <div class = "w-75 btn-group" role="group"> 
                        <a href="/admin/order/details?id=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width":"8%"
            }
        ]
    });
}