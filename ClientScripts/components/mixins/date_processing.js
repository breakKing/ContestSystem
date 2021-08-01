import moment from "moment";

export default {
    methods: {
        to_local(utc_time) {
            return moment.utc(utc_time).local()
        },
        to_utc(local_time) {
            return moment(local_time).utc()
        },
        to_utc_string(local_time) {
            return this.to_utc(local_time).format()
        },
        to_local_string(utc_time) {
            return this.to_local(utc_time).format().slice(0, 16)
        }
    }
}