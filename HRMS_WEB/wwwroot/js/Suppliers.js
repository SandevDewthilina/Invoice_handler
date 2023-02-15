import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            _suppliersList: []
        }
    },
    computed: {
        suppliers() {
            return this._suppliersList
        }
    },
    methods: {},
    created() {
        axios.get('/api/MasterDataApi/GetSuppliers').then(resp => {
            this._suppliersList = resp.data.data
            console.log(this._suppliersList)
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