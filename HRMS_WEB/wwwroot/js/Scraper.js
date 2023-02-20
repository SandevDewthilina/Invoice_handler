import {createApp} from '../lib/vue/dist/vue.esm-browser.js';

const app = createApp({
    data() {
        return {
            loading: false,
            scrapeData: {
                json: {
                    fields: [],
                    table: {
                        headings: [],
                        content: []
                    }
                }
            }
        }
    },
    computed: {
        getScrapedData() {
            return this.scrapeData
        },
        getJson() {
            return JSON.stringify(this.scrapeData.json, undefined, 2)
        },
        getFieldList() {
            let fieldList = []
            this.scrapeData.json.fields.forEach(item => {
                fieldList.push(
                    {
                        key: item.key,
                        value: item.value
                    }
                )
            })
            console.log(fieldList)
            return fieldList
        },
        getTableHeadings() {
            console.log(this.scrapeData.json.table.headings)
            return this.scrapeData.json.table.headings
        },
        getTableContent() {
            console.log(this.scrapeData.json.table.content)
            return this.scrapeData.json.table.content
        }
    },
    methods: {
        fetchTables() {
            this.loading = true
            // get pdf link for upload id
            const params = new Proxy(new URLSearchParams(window.location.search), {
                get: (searchParams, prop) => searchParams.get(prop),
            });

            const uploadId = params.Id

            axios.get('/api/UploadsApi/GetUploadForId?Id=' + uploadId).then(resp => {
                console.log(resp.data.data)
                const body = {
                    url: 'http://localhost:8100/' + resp.data.data.filePath,
                    filename: resp.data.data.fileName,
                    upload_name: resp.data.data.id
                }
                console.log(body)
                axios.post('/api/ScraperApi/ScrapeTableOfPdf', body).then(resp => {
                    this.loading = false
                    this.scrapeData.json.table = JSON.parse(resp.data)
                    console.log(this.scrapeData.json)
                }).catch(err => {
                    alert(err.message)
                    this.loading = false
                })
            }).catch(err => {
                alert(err.message)
                this.loading = false
            })
        },
        clickExport() {
            console.log('fdsf')
            let content = "data:text/csv;charset=utf-8,"
            // add fields
            this.scrapeData.json.fields.forEach(field => {
                let row = field.key + ',' + field.value + '\n'
                content += row
            })
            // add table to csv
            const headings = this.scrapeData.json.table.headings
            const rows = this.scrapeData.json.table.content
            // write headings
            content += headings.join() + '\n'
            // write content
            rows.forEach(row => {
                content += row.join() + '\n'
            })

            const encodedUri = encodeURI(content);
            window.open(encodedUri);
        }
    },
    created() {
        const params = new Proxy(new URLSearchParams(window.location.search), {
            get: (searchParams, prop) => searchParams.get(prop),
        });
        const uploadId = params.Id
        $('.extract-btn').click(() => {
            console.log('clicked')
            
            const templateId = $('#myselect').find(":selected").val()
            

            axios.get(`/api/ScraperApi/ScrapeUploadForTemplate?uploadId=${uploadId}&templateId=${templateId}`).then(resp => {
                this.scrapeData.json.fields = resp.data.data
                alert('Extraction Complete!')
            }).catch(err => {
                alert(err.message)
            })
        })
        
        axios.get('/api/UploadsApi/GetUploadDataForUploadId?Id=' + uploadId).then(resp => {
            const uploadData = resp.data.data
            console.log(uploadData)

            if (uploadData.fieldJson !== null) {
                // set field list
                JSON.parse(uploadData.fieldJson).forEach(item => {
                    this.scrapeData.json.fields.push(
                        {
                            key: item.Key,
                            value: item.Value
                        }
                    )
                })
            }
            
            // set table
            this.scrapeData.json.table = JSON.parse(uploadData.tableJson)
            
        }).catch(err => {
            alert('Data Prefetching failed')
        })
    }

});

app.mount("#sidepane");