var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    loadList(url);
});

function loadList(url) {
    dataTable = $('#DT_lo').DataTable({
        "ajax": {
            "url": "/api/amountbal" + url,
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "date", "width": "8%" },
            { "data": "sno", "width": "12%" },
            { "data": "cr", "width": "12%" },
            { "data": "dr", "width": "12%" },
            { "data": "balance", "width": "12%" },
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
function lockunlock(id) {
    $.ajax({
        type: 'POST',
        url: '/api/stock',
        data: JSON.stringify(id),
        contentType: "application/json",
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