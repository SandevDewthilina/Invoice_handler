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
        axios.get('/api/UploadsApi/GetFileUploads').then(resp => {
            let list = resp.data.data;
            list.forEach(item => {
                item.status = item.supplier_name !== null;
            })
            this._myUploadList = list
            $(document).ready(function () {
                $("#myuploadstable").DataTable({
                    "responsive": true,
                    "autoWidth": false,
                });
            });
        }).catch(err => {
            console.log(err.message)
        })
    }
});

app.mount("#main");