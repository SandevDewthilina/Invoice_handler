import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
import {DocStatus, LastChange, Document, MenuItem, SideMenuCategory, sideMenuMixin} from "./manage.js";

const app = createApp({
    data() {
        return {
            form: {
                test_pdf_url: '',
                template_name: '',
                template_type: '',
                detectContours: false,
                selectedSupplier: '',
                templateRegexList: [],
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
            this.form.templateRegexList.forEach(regex => {
                if (regex.isGoogleVision) {
                    regex.isArea = true
                }
            })
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
            if(regexList.length > 0) {
                lastId = regexList[regexList.length - 1].id
            }
            const newId = lastId + 1;
            regexList.push({
                id: newId,
                key: '',
                value: '',
                area: '',
                isArea: false,
                isGoogleVision: true
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
                link.setAttribute('download', 'image.png');
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
            console.log(id)
            const obj = this.form.templateRegexList.findIndex(e => e.id === id)
            this.form.templateRegexList.splice(obj, 1)
        },
        submit(e) {
            e.preventDefault()
            const params = new Proxy(new URLSearchParams(window.location.search), {
                get: (searchParams, prop) => searchParams.get(prop),
            });
            this.form.templateRegexList.forEach(regex => {
                if (regex.isArea && regex.area === '') {
                    alert('areas cannot be empty for key: ' + regex.key)
                    throw Error("areas can't be null")
                }
            })
            console.log(this.form)
            axios.post('/api/MasterDataApi/EditTemplate?Id=' + params.id, this.form).then(resp => {
                if(resp.data.success) {
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
        const params = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
        
        axios.get('/api/MasterDataApi/GetTemplateForId?Id=' + params.id).then(resp => {
            console.log(resp.data)
            this.form = resp.data.data.form
            $("#suppliers").val(this.form.selectedSupplier).change();
            // let suppliersDrop = document.getElementById("suppliers")
            // for (var i = 0; i < suppliersDrop.options.length; i++) {
            //     if (suppliersDrop.options[i].value === this.form.selectedSupplier) {
            //         suppliersDrop.options[i].selected = true;
            //         suppliersDrop.selectedIndex = i;
            //         break;
            //     }
            // }
        }).catch(err => {
            console.log(err.message)
            alert(err.message)
        })
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