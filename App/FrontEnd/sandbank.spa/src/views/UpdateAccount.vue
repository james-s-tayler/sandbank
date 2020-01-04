<template>
    <div>
        <b-loading :is-full-page="true" :active="!loadedMetadata"></b-loading>
        <PageTitle title="Update Account"></PageTitle>
        <div class="box">
        <div class="columns is-gapless">
            <div class="column">
                <section>
                    <b-field label="Nickname">
                        <b-input 
                            type="text"
                            :placeholder="account.displayName"
                            v-model="nickname"
                            @keyup.native.enter="updateAccount"
                            required>
                        </b-input>
                    </b-field>
                    <b-button @click="updateAccount" type="is-info" :loading="processing">Update account</b-button>
                </section>
            </div>
            <div class="column">
            </div>
            <div class="column">
            </div>
        </div>
        </div>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import Axios, { AxiosResponse } from 'axios';
import { Account } from '@/account';
import { accountStore } from '@/store/store';
import PageTitle from '@/components/PageTitle.vue';

@Component({
    components: {
        PageTitle,
    },
})
export default class UpdateAccount extends Vue {
    
    //https://source.unsplash.com/random/100x100

    private nickname!: string;
    private imageUrl!: string;
    private processing: boolean = false;


    private get account(): Account {
        const accountId: number = Number(this.$route.params.accountId);
        return this.$store.getters[`${accountStore}/accounts`].find((acc: Account) => acc.id === accountId);
    }

    private get loadedMetadata(): boolean {
        return this.$store.getters[`${accountStore}/loadedMetadata`];
    }

    private mounted() {
        this.$store.dispatch(`${accountStore}/getAccounts`, 
        { includeMetadata: true } );
    }

    private async updateAccount() {
        this.processing = true;

        const accountMetadata = {
            accountId: this.account.id,
            nickname: this.nickname,
            imageUrl: this.account.imageUrl, //update this
        };

        this.$store.dispatch(`${accountStore}/updateMetadata`, accountMetadata)
        .then((response: any) => {
            this.processing = false;
            this.$router.replace('/accounts');
        });
    }

}
</script>

<style scoped>

</style>


