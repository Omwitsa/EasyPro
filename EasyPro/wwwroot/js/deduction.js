var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    loadList(url);
});

function loadList(url) {
    dataTable = $('#DT_lo').DataTable({
        "ajax": {
            "url": "/api/expendituretrans" + url,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "dateDed", "width": "15%" },
            { "data": "expenditure.name", "width": "25%" },
            { "data": "amount", "width": "10%" },
            { "data": "remarks", "width": "15%" },
            { "data": "branch.name", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Meat Activities/expenditure/Upsert?Id=${data}" class="btn btn-success text-white">
                                 <i class="fa-solid fa-pen-to-square"></i></i> Edit</a>
                                <a onClick=Delete('/api/expendituretrans/'+${data}) class="btn btn-danger text-white" style="cursor:pointer;width:100px;">
                                 <i class="fa-solid fa-trash-can"></i> Delete</a>
                         </div>`;
                },
                "width": "30%",
            }
        ],
        "Language": {
            "emptyTable": "no data found."
        },
        "width": "100%",
        "order": [[0, "desc"]]
    });
}
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        //success notification
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        //failsure notification
                        toastr.error(data.message);
                    }
                }
            });
        }
    });

}