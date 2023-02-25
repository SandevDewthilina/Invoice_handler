import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            form: {
                test_pdf_url: '',
                template_name: '',
                templateRegexList: [
                    {
                        id: 1,
                        key: 'Supplier',
                        value: '',
                        area: '',
                        isArea: false
                    }
                ],
                tablesList: []
            }
        }
    },
    computed: {
        getForm() {
            return this.form
        },
        getName() {
            return this.form.template_name
        },
        getRegexComponents() {
            console.log(this.form.templateRegexList)
            return this.form.templateRegexList
        },
        getTableList() {
            return this.form.tablesList
        }
    },
    methods: {
        addRow() {
            let regexList = this.form.templateRegexList
            let lastId = -1;
            if (regexList.length > 0) {
                lastId = regexList[regexList.length - 1].id
            }
            const newId = lastId + 1;
            regexList.push({
                id: newId,
                key: '',
                value: '',
                area: '',
                isArea: false
            })
            this.form.templateRegexList = regexList
        },
        detectRow(id) {
            const obj = this.form.tablesList.find(e => e.id === id)
            console.log(obj)
            const req_body = {
                file_url: this.form.test_pdf_url,
                file_name: create_UUID() + '.pdf',
                upload_name: -1111,
                page_no: obj.page_no,
                flavor: obj.flavor,
                split_text: obj.split_text,
                edge_tol: obj.edge_tol,
                row_tol: obj.row_tol,
                table_areas: obj.area,
                flag_size: obj.font_sensitive,
                columns: obj.columns
            }
            console.log(req_body)
            axios.post('/api/ScraperApi/DetectAreaOfPdfUrl', req_body, {responseType: 'blob'}).then(resp => {
                const url = window.URL.createObjectURL(new Blob([resp.data]));
                const link = document.createElement('a');
                link.href = url;
                link.setAttribute('download', 'image.jpg');
                document.body.appendChild(link);
                link.click();
            }).catch(err => {
                alert(err.message)
            })
        },
        deleteTableRow(id) {
            console.log(id)
            const obj = this.form.tablesList.findIndex(e => e.id === id)
            this.form.tablesList.splice(obj, 1)
        },
        addTableRow() {
            let tableList = this.form.tablesList
            let lastId = -1;
            if (tableList.length > 0) {
                lastId = tableList[tableList.length - 1].id
            }
            let newId = lastId + 1
            this.form.tablesList.push({
                id: newId,
                page_no: '1',
                flavor: 'stream',
                edge_tol: 50,
                row_tol: 2,
                area: null,
                split_text: false,
                font_sensitive: false,
                columns: '',
                headings: ''
            })
        },
        deleteRow(id) {
            const obj = this.form.templateRegexList.findIndex(e => e.id === id)
            this.form.templateRegexList.splice(obj, 1)
        },
        submit(e) {
            e.preventDefault()
            let supplierElement = this.form.templateRegexList.find(e => e.key === 'Supplier')
            if (supplierElement.value === '') {
                alert("please enter regex for the Supplier Tag")
                return
            }
            console.log(this.form)
            axios.post('/api/MasterDataApi/CreateTemplate', this.form).then(resp => {
                if (resp.data.success) {
                    window.location.href = '/Home/ViewTemplates'
                } else {
                    alert('request was not successful')
                }
            }).catch(err => {
                console.log(err.message)
                alert(err.message)
            })
        }
    },
    created() {

    }
});

app.mount("#main");

function create_UUID(){
    var dt = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = (dt + Math.random()*16)%16 | 0;
        dt = Math.floor(dt/16);
        return (c=='x' ? r :(r&0x3|0x8)).toString(16);
    });
    return uuid;
}