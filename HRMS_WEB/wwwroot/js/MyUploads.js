import {createApp} from '../lib/vue/dist/vue.esm-browser.js';


const app = createApp({
    data() {
        return {
            _myUploadList: []
        }
    },
    computed: {
        myUploads() {
            return this._myUploadList
        }
    },
    methods: {
        setMyUploads(data) {
            this._myUploadList = data
        }
    },
    created() {
        axios.get('http://localhost:9100/api/GatewayApi/GetUploadList').then(resp => {
            let datalist = resp.data.data
            datalist.forEach(item => {
                item.status = item.supplier_name !== null
            })
            this._myUploadList = datalist
            console.log(this._myUploadList)
            $(document).ready(function () {
                $("#myuploadstable").DataTable({
                    "responsive": true,
                    "autoWidth": false,
                    "pageLength": 50,
                    "order": [ 0, 'desc' ]
                });
            });
        }).catch(err => {
            console.log(err.message)
        })
    }
});

app.mount("#main");