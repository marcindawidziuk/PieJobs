import {Store} from "./Store";
import {UserDetailsDto, UsersClient} from "../services/api.generated.clients";

interface UserData{
    user: UserDetailsDto | null;
    token: string | null;
}

export class UserStore extends Store<UserData>{
    constructor() {
        super();
    }
    protected data(): UserData {
        const token = localStorage.getItem("pie-jobs-token")
        return {
            user: null,
            token: token
        }
    }

    async refreshUser() {
        const client = new UsersClient();
        this.state.user = await client.getDetails()
    }

    logout(){
        this.state.user = null
        this.setToken(null)
    }

    setUser(user: UserDetailsDto){
        this.state.user = user
    }

    getUser(): UserDetailsDto | null{
        return this.state.user
    }

    isAuthenticated(): boolean{
        return !!this.state.token;
    }


    setToken(token: string | null){
        this.state.token = token
        localStorage.setItem("pie-jobs-token", token ?? "");
    }

}

export const userStore = new UserStore()
