import {JobStatus} from "~/services/api.generated.clients";

export const secondsBetweenDates = function (date1: Date, date2: Date){
    return (date2 - date1) / 1000;
}

export const statusDisplay = function (status: JobStatus){
    if (status == JobStatus.Pending)
        return "Pending"
    if (status == JobStatus.InProgress)
        return "In Progress"
    if (status == JobStatus.Completed)
        return "Completed"
    if (status == JobStatus.Cancelled)
        return "Cancelled"
    return "Failed"
}
