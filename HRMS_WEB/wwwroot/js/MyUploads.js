import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            _myUploadList: [
                {
                    id: 1,
                    thumbnail: '~/assets/179483.png',
                    filename: 'Invoice 2023 august.pdf',
                    progress: 0.80,
                    status: 'pending',
                    upload_date: '2023/02/15',
                    supplier_name: 'Kaleni Cables Pvt Ltd'
                },
                {
                    id: 2,
                    thumbnail: '~/assets/179483.png',
                    filename: 'Annual Balance Sheet.pdf',
                    progress: 0.20,
                    status: 'success',
                    upload_date: '2021/12/02',
                    supplier_name: 'Samanmal Stores'
                }
            ]
        }
    },
    computed: {
        myUploads() {
            return this._myUploadList
        }
    },
    methods: {}
});

app.mount("#main");